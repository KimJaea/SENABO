package com.senabo.domain.affection.repository;

import com.senabo.domain.affection.entity.Affection;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.domain.Sort;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.web.SortDefault;
import org.springframework.stereotype.Repository;

import java.time.LocalDateTime;
import java.util.List;

@Repository
public interface AffectionRepository extends JpaRepository<Affection, String>, AffectionRepositoryCustom {
    List<Affection> deleteByMemberId(Member memberId);
}