package com.senabo.domain.member.repository;

import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface MemberRepository extends JpaRepository<Member, String>, MemberRepositoryCustom {
    Optional<Member> findById(Long memberId);

    void deleteById(Long id);

    Boolean existsByEmail(String email);

    Optional<Member> findByEmail(String email);

    void deleteByEmail(String email);
}
