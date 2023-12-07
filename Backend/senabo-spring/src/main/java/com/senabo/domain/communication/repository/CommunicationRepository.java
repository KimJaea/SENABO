package com.senabo.domain.communication.repository;

import com.senabo.domain.communication.entity.Communication;
import com.senabo.domain.member.entity.Member;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;


@Repository
public interface CommunicationRepository extends JpaRepository<Communication, String>, CommunicationRepositoryCustom {
    List<Communication> deleteByMemberId(Member memberId);

}
